#!/bin/bash
#
# https://eff-certbot.readthedocs.io/en/stable/using.html#pre-and-post-validation-hooks
#
# Passed environment variables:
#
# $CERTBOT_DOMAIN The domain being authenticated
# $CERTBOT_VALIDATION The validation string
# $CERTBOT_TOKEN Resource name part of the HTTP-01 challenge (HTTP-01 only)
# $CERTBOT_REMAINING_CHALLENGES Number of challenges remaining after the current challenge
# $CERTBOT_ALL_DOMAINS A comma-separated list of all domains challenged for the current certificate
#
# Usage:
#
# Ensure that /etc/heracles.conf has been created
#
# certbot certonly --manual --preferred-challenges=dns \
#                  --manual-auth-hook /path/to/CertBotAuth.sh \
#                  --manual-cleanup-hook /path/to/CertBotCleanup.sh
#
# To Do:
#
# - Handle multi-domain certificate requests

DNS_SERVERS=("129.67.1.1" "129.67.1.180" "163.1.2.1" "1.1.1.1" "8.8.8.8" "9.9.9.9" "208.67.222.222")
LOGFILE="/var/log/HeraclesCertBotAuth.log"
date > "$LOGFILE"
chmod 600 "$LOGFILE"
exec 3>>"$LOGFILE"
BASH_XTRACEFD=3
set -x

DNS_UPDATE_DELAY=900
CREATE_DOMAIN="_acme-challenge.$CERTBOT_DOMAIN"
HERACLES_STDIN="[{ \"comment\": \"WinAcme\", \"content\": \"$CERTBOT_VALIDATION\", \"hostname\": \"$CREATE_DOMAIN.\", \"type\": \"TXT\" }]"

Panic () {
	echo "$1"
	exit 1
}

CheckBin () {
	which "$1" &>/dev/null || Panic "Cannot find executable $1"
}

CheckAcmeTxtRecordExists () {
	for DNS_SERVER in "${DNS_SERVERS[@]}"; do
		RES=$(dig @$DNS_SERVER  +noall +answer TXT "$CREATE_DOMAIN")
		if [[ -z $RES ]]; then
			return 1
		fi
	done
	return 0
}

CheckAcmeTxtRecordRemoved () {
	for DNS_SERVER in "${DNS_SERVERS[@]}"; do
		RES=$(dig @$DNS_SERVER  +noall +answer TXT "$CREATE_DOMAIN")
		if [[ -n $RES ]]; then
			return 1
		fi
	done
	return 0
}

GetAcmeTxtRecordContents () {
	dig +noall +answer TXT "$CREATE_DOMAIN" | cut -d '"' -f 2
}

DoCheckCreate () {
	sleep 60
	CheckAcmeTxtRecordExists || (echo "Record not yet in DNS" && DoCheckCreate)
	CURRENT=$(GetAcmeTxtRecordContents)
	if [ "$CURRENT" != "$CERTBOT_VALIDATION" ]; then
		echo "$CURRENT != $CERTBOT_VALIDATION"
		DoCheckCreate
	fi
}

DoCheckDelete () {
	sleep 60
	CheckAcmeTxtRecordRemoved || (echo "Record still in DNS" && DoCheckDelete)
}

DoCleanup () {
	CertBotCleanup.sh $CERTBOT_DOMAIN || Panic "Hydra DNS delete failed"
	DoCheckDelete
}

DoCreate () {
	CheckAcmeTxtRecordExists && DoCleanup
	CheckAcmeTxtRecordExists && Panic "Hydra stale DNS record found"
	echo "$HERACLES_STDIN" | heracles add || Panic "Hydra DNS update failed"
	DoCheckCreate
}

CheckBin dig
CheckBin nslookup
CheckBin heracles

source /etc/heracles.conf || Panic "Cannot open /etc/heracles.conf"
if [ -z "$HYDRA_URI" ]; then
  Panic "HYDRA_URI is not set"
fi
if [ -z "$HYDRA_TOKEN" ]; then
  Panic "HYDRA_TOKEN is not set"
fi

nslookup "$CERTBOT_DOMAIN" &> /dev/null || Panic "$CERTBOT_DOMAIN is not a valid DNS entry"
DoCreate

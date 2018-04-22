#!/bin/sh

set -e

exec sh -c \
  "exec varnishd -F -u varnish \
  -f $VCL_CONFIG \
  -s malloc,$CACHE_SIZE \
  $VARNISHD_PARAMS"
#!/bin/bash
set -e
# echo "processing params"
while [ "$1" != "" ]; do
    case $1 in
        --ancestor)
            ANCESTOR=$2
            shift 2
            ;;
        *)
            PARAMS="$PARAMS$1 "
            shift 1
            ;;
    esac
done
echo "params is $PARAMS";
echo "ancestor is $ANCESTOR";
CONTAINER=`docker ps --filter "ancestor=$ANCESTOR" --format '{{.Names}}'`;
echo "starting debugger on $CONTAINER";
docker exec -i $CONTAINER $PARAMS;
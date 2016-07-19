#!/usr/bin/env bash

wget http://dev.mysql.com/get/mysql-apt-config_0.3.2-1ubuntu14.04_all.deb
dpkg -i mysql-apt-config_0.3.2-1ubuntu14.04_all.deb
apt-get update
apt-get install mysql-server-5.7

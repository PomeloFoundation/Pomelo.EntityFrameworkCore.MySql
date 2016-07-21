#!/usr/bin/env bash

wget http://dev.mysql.com/get/mysql-apt-config_0.6.0-1_all.deb

sudo dpkg -i mysql-apt-config_0.6.0-1_all.deb

sudo apt-get -y remove mysql-server

sudo apt-get -y remove mysql-client

sudo apt-get -y autoremove

sudo apt-get -y update

sudo apt-get -y install mysql-server-5.7

mysqladmin -u root -p password Password12!


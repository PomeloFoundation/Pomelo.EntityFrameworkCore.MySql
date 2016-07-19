#!/usr/bin/env bash

wget http://dev.mysql.com/get/mysql-apt-config_0.6.0-1_all.deb
sudo dpkg -i mysql-apt-config_0.6.0-1_all.deb
sudo apt-get remove mysql-server
sudo apt-get remove mysql-client
sudo apt-get autoremove
sudo apt-get update
sudo apt-get install mysql-server-5.7
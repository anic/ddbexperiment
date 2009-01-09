@echo off
start "S1" LocalSite.exe NetworkInitScript.txt S1
start "S2" LocalSite.exe NetworkInitScript.txt S2
start "S3" LocalSite.exe NetworkInitScript.txt S3
start "S4" LocalSite.exe NetworkInitScript.txt S4

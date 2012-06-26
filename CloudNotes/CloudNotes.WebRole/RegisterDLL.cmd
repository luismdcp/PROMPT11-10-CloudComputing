@echo off
sc config wuauserv start=demand
wusa.exe "Windows6.0-KB974405-x64.msu" /quiet /norestart
sc config wuauserv start= disabled
exit /b 0
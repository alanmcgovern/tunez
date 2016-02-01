all: nuget-restore
	xbuild

all-server: nuget-restore
	xbuild /property:Configuration=DebugServer

nuget-restore:
	nuget restore

run-server:
	mono --debug Server/TunezServer/bin/Debug/TunezServer.exe


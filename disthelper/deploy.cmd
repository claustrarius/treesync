SET TREESYNC_BIN=.\TreeSync.exe
SET SOURCEPATH=./publish/
SET TARGETPATH=%1
SET CONFIGFILE=./treesync-config.json
SET IGNOREFILE=./treesync-ignore.txt

%TREESYNC_BIN% --source %SOURCEPATH% --target %TARGETPATH% --config %CONFIGFILE% --ignore %IGNOREFILE% %2 %3 %4 %5 %6 %7 %8 %9

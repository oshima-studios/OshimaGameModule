git clone https://github.com/project-redbud/FunGame-Core
git clone https://github.com/project-redbud/FunGame-Server
git clone https://github.com/oshima-studios/OshimaGameModule
git clone https://github.com/project-redbud/SQLQueryExtension
ren "FunGame-Core" "FunGame.Core"
ren "FunGame-Server" "FunGame.Server"
if not exist "FunGame.Extension" mkdir "FunGame.Extension"
move "SQLQueryExtension" "FunGame.Extension\FunGame.SQLQueryExtension"
call "start-web-api.bat"
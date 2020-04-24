cd debug/Master
start OpenNos.Master.Server.exe
timeout 3

cd ..
cd ..
cd dist/DiscordBot
start GloomyTale.DiscordBot.exe

cd..
cd..

cd debug/World
start OpenNos.World.exe
timeout 2

cd..

cd World
start OpenNos.World.exe
timeout 2

cd..

cd World
start OpenNos.World.exe
timeout 2

cd ..

cd FrozenCrown
start OpenNos.World.exe
timeout 15

cd ..

cd Login
start OpenNos.Login.exe

exit
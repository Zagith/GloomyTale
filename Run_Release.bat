cd dist/Master
start OpenNos.Master.Server.exe
timeout 10

cd ..

cd World
start OpenNos.World.exe
timeout 5000

cd ..

cd Login
start OpenNos.Login.exe

exit
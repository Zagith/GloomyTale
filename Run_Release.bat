cd OpenNos.Master.Server
start OpenNos.Master.Server.exe
timeout 30

cd ..

cd OpenNos.World

cd CH1
start OpenNos.World.exe
timeout 10

cd ..

cd CH2
start OpenNos.World.exe
timeout 10

cd ..

cd CH3
start OpenNos.World.exe
timeout 10

cd ..

cd CH4
start OpenNos.World.exe
timeout 10

cd ..

cd CH5
start OpenNos.World.exe
timeout 10

cd ..

::cd CH6
::start OpenNos.World.exe
::timeout 10

::cd ..

cd CH51
start OpenNos.World.exe --port 5100
timeout 10

cd ..
cd ..

cd OpenNos.Login
start OpenNos.Login.exe

exit
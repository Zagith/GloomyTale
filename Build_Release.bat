rd /s /q Release
mkdir Release

::xcopy /y /e /s /i OpenNos.ChatLog.Client\bin\Release Release\OpenNos.ChatLog.Client
::xcopy /y /e /s /i OpenNos.ChatLog.Networking\bin\Release Release\OpenNos.ChatLog.Networking
::xcopy /y /e /s /i OpenNos.ChatLog.Server\bin\Release Release\OpenNos.ChatLog.Server
::xcopy /y /e /s /i OpenNos.ChatLog.Shared\bin\Release Release\OpenNos.ChatLog.Shared
::xcopy /y /e /s /i OpenNos.ChatLog.Standalone\bin\Release Release\OpenNos.ChatLog.Standalone
::xcopy /y /e /s /i OpenNos.Core\bin\Release Release\OpenNos.Core
::xcopy /y /e /s /i OpenNos.DAL\bin\Release Release\OpenNos.DAL
::xcopy /y /e /s /i OpenNos.DAL.DAO\bin\Release Release\OpenNos.DAL.DAO
::xcopy /y /e /s /i OpenNos.DAL.EF\bin\Release Release\OpenNos.DAL.EF
::xcopy /y /e /s /i OpenNos.DAL.Interface\bin\Release Release\OpenNos.DAL.Interface
::xcopy /y /e /s /i OpenNos.Data\bin\Release Release\OpenNos.Data
::xcopy /y /e /s /i OpenNos.Domain\bin\Release Release\OpenNos.Domain
::xcopy /y /e /s /i OpenNos.GameObject\bin\Release Release\OpenNos.GameObject
::xcopy /y /e /s /i OpenNos.GameObject.Mock\bin\Release Release\OpenNos.GameObject.Mock
::xcopy /y /e /s /i OpenNos.Handler\bin\Release Release\OpenNos.Handler
xcopy /y /e /s /i OpenNos.Import.Console\bin\Release Release\OpenNos.Import.Console
xcopy /y /e /s /i OpenNos.Login\bin\Release Release\OpenNos.Login
::xcopy /y /e /s /i OpenNos.MallServiceExample\bin\Release Release\OpenNos.MallServiceExample
::xcopy /y /e /s /i OpenNos.Mapper\bin\Release Release\OpenNos.Mapper
::xcopy /y /e /s /i OpenNos.Master.Library\bin\Release Release\OpenNos.Master.Library
xcopy /y /e /s /i OpenNos.Master.Server\bin\Release Release\OpenNos.Master.Server
::xcopy /y /e /s /i OpenNos.PathFinder\bin\Release Release\OpenNos.PathFinder
::xcopy /y /e /s /i OpenNos.QuestGenerator.CLITest\bin\Release Release\OpenNos.QuestGenerator.CLITest
::xcopy /y /e /s /i OpenNos.Test\bin\Release Release\OpenNos.Test
::xcopy /y /e /s /i OpenNos.XMLModel\bin\Release Release\OpenNos.XMLModel

xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH1 /exclude:exclude.txt
xcopy /y /e /s /i Config\CH1 Release\OpenNos.World\CH1

xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH2 /exclude:exclude.txt
xcopy /y /e /s /i Config\CH2 Release\OpenNos.World\CH2

xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH3 /exclude:exclude.txt
xcopy /y /e /s /i Config\CH3 Release\OpenNos.World\CH3

xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH4 /exclude:exclude.txt
xcopy /y /e /s /i Config\CH4 Release\OpenNos.World\CH4

xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH5 /exclude:exclude.txt
xcopy /y /e /s /i Config\CH5 Release\OpenNos.World\CH5

::xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH6 /exclude:exclude.txt
::xcopy /y /e /s /i Config\CH6 Release\OpenNos.World\CH6

xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH51 /exclude:exclude.txt
xcopy /y /e /s /i Config\CH51 Release\OpenNos.World\CH51

copy Run_Release.bat Release\Run.bat
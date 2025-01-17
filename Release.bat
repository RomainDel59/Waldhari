@echo off

if exist ".\Release" (
	rmdir /s /q ".\Release"
)

mkdir ".\Release"

copy /y ".\Waldhari\bin\Release\Waldhari.dll" ".\Release\" 

xcopy /e /i /y ".\Waldhari\Properties\Waldhari" ".\Release\Waldhari"

::tar -acf ".\Release\MethLab.zip" ".\Release\*"
powershell -Command "Compress-Archive -Path '.\Release\*' -DestinationPath '.\Release\Waldhari.zip'"

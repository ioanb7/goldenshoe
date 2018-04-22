#Call this after you make changes to messages and have rebuilt the project

$TARGETDIR = "C:\Users\$env:UserName\.nuget\packages\ioanb7.messages"
if(Test-Path -Path $TARGETDIR ){
    rd -r $TARGETDIR
}
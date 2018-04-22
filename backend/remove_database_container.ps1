Write-host  ("Searching for 'mariadb' containers")

$OutputVariable = (docker container ls) | Out-String
$array = $OutputVariable.Split([Environment]::NewLine)

$matches = 0
$last_match = ""

for ($i=0; $i -lt $array.length; $i++) {
    $string = $array[$i]
    if ($string -like '*mariadb*') {
        $array2 = $string.Split(' ')
        $last_match = $array2[0]
        Write-host  ("Found: {0}" -f $last_match)
        $matches = $matches + 1
    }
}

if ($matches -eq 1) {
    Write-host ("Would you like to delete {0} ? (Default is No)" -f $last_match )-ForegroundColor Yellow 
    $Readhost = Read-Host " ( y / n ) " 
    Switch ($ReadHost) 
     { 
       Y {Write-host "Yes"; $ToDelete=$true} 
       N {Write-Host "No"; $ToDelete=$false} 
       Default {Write-Host "Default, No"; $ToDelete=$false} 
     }
     
     if($ToDelete) {
        docker container rm $last_match
     }
}
if ($matches -lt 1) {
    Write-host  ("Found < 0 matches")
}
if ($matches -gt 1) {
    Write-host  ("Found > 0 matches")
}
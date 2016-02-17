Get-ChildItem *.cs -recurse |
    Foreach-Object {
        $c = ($_ | Get-Content) 
        $c = $c -replace '<add key="Environment" value="Dev"/>','<add key="Environment" value="Demo"/>'
        [IO.File]::WriteAllText($_.FullName, ($c -join "`r`n"))
    }
Get-ChildItem .\ -include obj,bin -Recurse | foreach ($_) {foreach($i in Get-ChildItem $_.fullname){ remove-item $i.fullname -Force -Recurse}}

# Finds aseprite in its normal install destinations in order to run it. Works on macOS and Windows.
function aseprite {
    $steamPath = "~/Library/Application Support/Steam/steamapps/common/Aseprite/Aseprite.app/Contents/MacOS/aseprite"
    $normalPath = "/Applications/Aseprite.app/Contents/MacOS/aseprite"
    if ($IsWindows) {
        $steamPath = "C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe"
        $normalPath = "C:\Program Files (x86)\Aseprite\Aseprite.exe"
    }

    if (Test-Path $steamPath) {
        . $steamPath $args
    } elseif (Test-Path $normalPath) {
        . $normalPath $args
    } else {
        throw "Please install aseprite before using the aseprite command"
    }
}

# Exports the given .aseprite file as a horizontal .png spritesheet.
function Export-SimpleSprite($filePath) {
    echo "Exporting $($filePath)"
    $extension = Get-Item $filePath | Select-Object Extension
    $fileName = Resolve-Path $filePath | Split-Path -leaf
    $exportName = $fileName.Substring(0, $fileName.Length - $extension.Extension.Length) 
    aseprite -b $filePath --sheet "$($assetsFolder)/$($exportName).png" --sheet-type horizontal
}

# Find relevant directories
$rootFolder = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$assetsFolder = "$($rootFolder)/src/assets"

# Export all files
Write-Output "Starting aseprite spritesheet exporting."
Get-ChildItem -Filter "*.aseprite" -File "$($assetsFolder)" | ForEach-Object { Export-SimpleSprite $_ }
Write-Output "Finished exporting succesfully."

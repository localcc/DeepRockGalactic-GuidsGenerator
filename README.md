# DeepRockGalactic - Guids Generator

Software that is used to generate `guids.json` used by [DeepRockGalactic save editor](https://github.com/localcc/deeprockgalactic-saveeditor).

## Why?
When DeepRockGalactic releases an update, this file should be regenerated for new overclocks to be available for editing in save editors.

## Usage
- Download newest release from [releases]() page
- Unpack DeepRockGalactic game files using [DRGPackerv2](https://github.com/DRG-Modding/Useful-Scripts/)
- Run Guids generator with path to unpacked game files like this
```
GuidsGenerator.exe unpacked/FSD/Content
```
- With any luck, after the software finishes you should have a `guids.json` file created
- Paste the file in the folder with save editor executable and enjoy your new and shiny overclocks

## Building

### Requirements:
- .NET 5 SDK
- Git

```
git clone https://github.com/localcc/DeepRockGalactic-GuidsGenerator
cd DeepRockGalactic-GuidsGenerator
git submodule update --init --recursive
dotnet build
```
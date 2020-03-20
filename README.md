# hydrologyVR

The prototype for interactive exploration of hydrology models in VR.
In particular, we're trying to show how yearly rainfall and current forest cover affect the runoff of downstream pollutants in the South Nation Watershed.

For pre-built binaries see [Releases](https://github.com/kevinkle/hydrologyVR/releases).

# Quickstart

We're using Unity and building for the Gear VR platform.
You'll need a Gear VR headset and a compatible Samsung device.
To start, you'll need to enable installation "Unknown Sources" on your device and then, on your device, navigate to [Releases](https://github.com/kevinkle/hydrologyVR/releases) to download the APK.

# Development

You'll need to install:
* Unity
* Oculus SDK
* JDK
* Android SDK

To develop the Unity application, clone this project and open it in Unity.
For the Watershed API, you can also clone with the `--recursive` option to clone the Watershed project as well.
If you're only working on the Unity application, then we also provide a hosted version of the Watershed API.

# Architecture

## Unity

* Oculus SDK: for VR development for Gear VR
* Mapbox API: for 3D satellite rendering of terrain

## Watershed API

* Separate project for doing the waterhsed modelling; has its own repository with details

# Docs

For additional documentation, see the [Wiki](https://github.com/kevinkle/hydrologyVR/wiki)

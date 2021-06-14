# Prerequisites

## Prerequisites to develop on your host machine

- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli), then run `az login`
- [Pulumi](https://www.pulumi.com/docs/get-started/install/) >= 3.3.1, then run `pulumi login --local`
- [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0) >= 5.0.202
- [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) (on windows) or [Azurite](https://github.com/azure/azurite) (on Linux)
- [Visual Studio Code](https://code.visualstudio.com/Download)

## Prerequisites to develop inside the provided `.devcontainer`

You need to have docker and [Visual Studio Code](https://code.visualstudio.com/Download) installed on your host machine. 

If you choose to use the provided `.devcontainer` for development instead of installing the dependencies to your host machine, you only need to run `az login` and `pulumi login --local` from inside the `.devcontainer`. 

The `.devcontainer` can be started from Visual Studio Code by 
- hitting `F1`
- then selecting `>Remote-Containers: Open Folder in Container...`
- opening the `lab-dev-env` root directory. 

All dependencies are already pre-installed in the `.devcontainer`.

In order to run the `.devcontainer`, you need to have Docker installed, see https://code.visualstudio.com/docs/remote/containers#_system-requirements for details.

Know issues / open points:
- Pre-install Java and GraphViz
- Add `npm i` to the Dockerfile for the UI 
- Add running Azurite to the Dockerfile
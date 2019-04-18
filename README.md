<<<<<<< HEAD
# AVM (Azure Variable Manager)

This a simple command line tool for managing Azure variables in masses instead of clicking on UI one by one.

## Usage

### Environment variables

The following environment variables must be configured to use AVM

- `AVM_TokenPath` Path to a file containing the Azure DevOps Personal Access Token.
- `AVM_Organization` Azure DevOps organization name the project is in.
- `AVM_Project` Azure DevOps project name.

### Commands

#### List builds and releases

- `AVM list builds` lists all available builds
- `AVM list releases` lists all available releases

#### Get build or release

- `AVM get build <build id>` returns specified build
- `AVM get release <release id>` returns specified release

#### Update build or release

- `AVM set build <build id> -s <source file path> -c <comment>` updates specified build using the JSON from source file path.
- `AVM set release <build id> -s <source file path> -c <comment>` updates specified release using the JSON from source file path.
=======
# avm
Azure Variable Manager is a tool for managing variables in a more sensible way
>>>>>>> b70652b7d1182e3762aafa82537d8c2d9ac46c7b

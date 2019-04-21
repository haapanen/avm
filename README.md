# AVM (Azure Variable Manager)
[![Build Status](https://haapanen.visualstudio.com/AVM/_apis/build/status/haapanen.avm?branchName=master)](https://haapanen.visualstudio.com/AVM/_build/latest?definitionId=5&branchName=master)
# Work in progress, most features are not implemented

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

#### Get variables for build or release

- `AVM get variables -b <build id>` returns variables for specified build
- `AVM set variables -b <build id> -s <source file path> -c <comment>` updates variables for specified build

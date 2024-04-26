# Tips for running linux unit tests.

> Run these commands from the \{RepositoryRoot\}\Source directory

## Build the image from the Dockerfile

```cmd
docker build -t perfutils-linux-tests -f linuxtest.dockerfile .
```

## Create a host machine folder for the test results

This folder will be used to store the test results from the container so you can review them later.

```cmd
mkdir {/path/to/host/machine/folder}
```

> Replace _\{/path/to/host/machine/folder\}_ with the path to the folder on the host machine you want to store the test result files in.

## Run the container from the image

This will run the tests and store the results in the folder you created in the previous step.

```cmd
docker run --rm -v {/path/to/host/machine/folder}://app/Tst/linux-tests-results perfutils-linux-tests
```

## Review the test results

The test result files are stored in the folder you specified in the previous step (\{/path/to/host/machine/folder\}) in xUnit XML format.


## Cleanup the image when done with tests

Keep your environment clean by removing the image when you are done running tests.

```cmd
docker rmi perfutils-linux-tests
```

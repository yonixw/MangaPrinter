cd "%solutiondir%"

REM Make sure `git` is in the PATH.

FOR /F "tokens=*" %%g IN ('git rev-parse --abbrev-ref HEAD') do (
	SET branch=%%g
)

FOR /F "tokens=* usebackq" %%g IN (`git show --no-patch --format"=%%h, %%ci; %%f"`) do (
	SET commitinfo=%%g
)

FOR /F "tokens=* usebackq" %%g IN (`git describe --abbrev"=0" --tags`) do (
	SET latesttag=%%g
)

FOR /F "tokens=*" %%g IN ('git rev-list --count %latesttag%..') do (
	SET commitsafter=%%g
)

FOR /F "tokens=*" %%g IN ('git status --porcelain') do (
	SET dirty=*
)

mkdir "%projectdir%Resources"
echo "%latesttag%+%commitsafter%%dirty% (%branch%) %commitinfo%" > "%projectdir%Resources\GitInfo.txt"	
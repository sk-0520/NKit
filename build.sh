#!💩

# CIで止まられるとなんもできん
if [ -z "${CI+x}" ] ; then
    if [ `git status -s | wc -l` -ne 0 ] ; then
        git status
        echo "There is changed files. Press Any key to exit ... "
        read
        exit 1
    fi
else
    echo "[CI] skip: status check"
fi

# バージョン書き換え
VERSION_REV=`git rev-parse HEAD`
find -name 'AssemblyInfo.cs' -print0 | xargs -0 sed -E -i "s/^\[\s*assembly\s*:\s*\AssemblyInformationalVersion\s*\(\s*\"\s*(xxxxxxxxxxxxxxDEVELOPMENTxxxxxxxxxxxxxxx)\s*\"\s*\)\s*\]/[assembly: AssemblyInformationalVersion(\"$VERSION_REV\")]/"
# <YEAR>書き換え
find -name 'AssemblyInfo.cs' -print0 | xargs -0 sed -E -i "s/<YEAR>/`date +%Y`/"

# ビルド
pushd Build
    cmd.exe //c build.bat ${BUILD_TYPE} ${BUILD_OUTPUT}
popd

echo create tag api json
VERSION_NUM=`head -n 1 Output/Release/version.txt | tr -d " "`
echo "{ \"name\": \"ver_${VERSION_NUM//-/.}\", \"target\": { \"hash\": \"${VERSION_REV}\" } }" > Output/Release/bitcuket-tag.json

# バージョン戻し
git reset --hard

echo ""
if [ -f Build/@error ] ; then
    if [ -z "${CI+x}" ] ; then
        echo "!!!build failed!!! Press Any key to exit ..."
        read
    else
        echo "!!!build failed!!! CI mode."
        exit 1
    fi
else
    if [ -z "${CI+x}" ] ; then
        echo "build success. Press Any key to exit ..."
        read
    else
        echo "build success. CI mode."
    fi
fi


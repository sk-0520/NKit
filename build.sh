#!π©

# CIγ§ζ­’γΎγγγγ¨γͺγγγ§γγ
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

# γγΌγΈγ§γ³ζΈγζγ
VERSION_REV=`git rev-parse HEAD`
find -name 'AssemblyInfo.cs' -print0 | xargs -0 sed -E -i "s/^\[\s*assembly\s*:\s*\AssemblyInformationalVersion\s*\(\s*\"\s*(xxxxxxxxxxxxxxDEVELOPMENTxxxxxxxxxxxxxxx)\s*\"\s*\)\s*\]/[assembly: AssemblyInformationalVersion(\"$VERSION_REV\")]/"
# <YEAR>ζΈγζγ
find -name 'AssemblyInfo.cs' -print0 | xargs -0 sed -E -i "s/<YEAR>/`date +%Y`/"

# γγ«γ
pushd Build
    cmd.exe //c build.bat ${BUILD_TYPE} ${BUILD_OUTPUT}
popd

echo create tag api json
VERSION_NUM=`head -n 1 Output/Release/version.txt | tr -d " "`
echo "{ \"name\": \"ver_${VERSION_NUM//-/.}\", \"target\": { \"hash\": \"${VERSION_REV}\" } }" > Output/Release/bitcuket-tag.json

# γγΌγΈγ§γ³ζ»γ
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


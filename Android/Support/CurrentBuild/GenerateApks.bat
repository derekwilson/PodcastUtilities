del podcastutilities.apks
del podcastutilities.apks.zip
del universal.apk
del toc.pb
java -jar bundletool-all-1.9.1.jar build-apks --bundle=com.andrewandderek.podcastutilities-Signed.aab --output=podcastutilities.apks --mode=universal
ren podcastutilities.apks podcastutilities.apks.zip
PowerShell -ExecutionPolicy Unrestricted -command "Expand-Archive podcastutilities.apks.zip ."
pause

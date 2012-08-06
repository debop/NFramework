@echo off
@echo =========================================
@echo 모든 프로젝트를 release 모드로 빌드합니다.
@echo 테스트 Assembly도 빌드하려면 buildall 전에 test 를 추가하세요.
@echo =========================================
cd ..
nant quick release-all-frameworks > Build.log
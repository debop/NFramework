@echo off
@echo =========================================
@echo 프로젝트를 Release Mode로 빌드하고, Documents를 생성합니다.
@echo =========================================
cd ..
NAnt quick doxygen > Build.log
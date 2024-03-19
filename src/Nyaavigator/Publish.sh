#!/bin/bash

NC='\033[0m'
RED='\033[0;31m'
BLUE='\033[0;34m'

mainloop() {
  clear
  while true; do
    read -p "Choose platform (1 - Linux, 2 - Windows): " platformselect

    if [[ $platformselect -eq 1 ]]; then
      rid="linux-x64"
      framework="net8.0"
    elif [[ $platformselect -eq 2 ]]; then
      rid="win-x64"
      framework="net8.0-windows"
    else
      clear
      echo -e "${RED}Invalid platform selection. Please choose 1 or 2.${NC}"
      echo
      continue
    fi
    clear

    break
  done

  while true; do
    read -p "Choose configuration (1 - Release, 2 - Portable): " config

    if [[ $config -eq 1 ]]; then
      config="Release"
    elif [[ $config -eq 2 ]]; then
      config="Portable"
    else
      clear
      echo -e "${RED}Invalid configuration selection. Please choose 1 or 2.${NC}"
      echo
      continue
    fi
    clear

    break
  done

  echo -e "Executing: dotnet publish Nyaavigator.csproj -c ${BLUE}$config${NC} -r ${BLUE}$rid${NC} -f ${BLUE}$framework${NC} -o bin/Publish/${BLUE}$rid${NC}/${BLUE}$config${NC} --self-contained -p:PublishSingleFile=true"
  echo
  read -n1 -r -p "Press any key to continue..."
  clear
  dotnet publish Nyaavigator.csproj -c $config -r $rid -f $framework -o bin/Publish/$rid/$config --self-contained -p:PublishSingleFile=true
  echo
}

while true; do
  (mainloop)
  read -p "Restart (Y)/N? " choice
  if [[ $choice =~ ^[Nn]$ ]]; then
    break
  fi
done

exit 0

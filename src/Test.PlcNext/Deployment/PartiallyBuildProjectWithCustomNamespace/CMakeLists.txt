﻿cmake_minimum_required(VERSION 3.27)

project(PrjN_ST_Update_Proj_Targets_2237)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

################# create target #######################################################

set (WILDCARD_SOURCE *.cpp)
set (WILDCARD_HEADER *.h *.hpp *.hxx)

file(GLOB_RECURSE Headers CONFIGURE_DEPENDS src/${WILDCARD_HEADER} intermediate/code/${WILDCARD_HEADER})
file(GLOB_RECURSE Sources CONFIGURE_DEPENDS src/${WILDCARD_SOURCE} intermediate/code/${WILDCARD_SOURCE})
add_library(PrjN_ST_Update_Proj_Targets_2237 SHARED ${Headers} ${Sources})

#######################################################################################

################# project include-paths ###############################################

target_include_directories(PrjN_ST_Update_Proj_Targets_2237
    PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/intermediate/code>
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/src>)

#######################################################################################

################# include arp cmake module path #######################################

list(INSERT CMAKE_MODULE_PATH 0 "${ARP_TOOLCHAIN_CMAKE_MODULE_PATH}")

#######################################################################################

################# set link options ####################################################
# WARNING: Without --no-undefined the linker will not check, whether all necessary    #
#          libraries are linked. When a library which is necessary is not linked,     #
#          the firmware will crash and there will be NO indication why it crashed.    #
#######################################################################################

target_link_options(PrjN_ST_Update_Proj_Targets_2237 PRIVATE LINKER:--no-undefined)

#######################################################################################

################# add link targets ####################################################

find_package(ArpDevice REQUIRED)
find_package(ArpProgramming REQUIRED)

target_link_libraries(PrjN_ST_Update_Proj_Targets_2237 PRIVATE ArpDevice ArpProgramming)

#######################################################################################

################# install ############################################################

string(REGEX REPLACE "^.*\\(([0-9]+\.[0-9]+\.[0-9]+\.[0-9]+).*$" "\\1" _ARP_SHORT_DEVICE_VERSION ${ARP_DEVICE_VERSION})
install(TARGETS PrjN_ST_Update_Proj_Targets_2237
    LIBRARY DESTINATION ${ARP_DEVICE}_${_ARP_SHORT_DEVICE_VERSION}/$<CONFIG>/lib
    ARCHIVE DESTINATION ${ARP_DEVICE}_${_ARP_SHORT_DEVICE_VERSION}/$<CONFIG>/lib
    RUNTIME DESTINATION ${ARP_DEVICE}_${_ARP_SHORT_DEVICE_VERSION}/$<CONFIG>/bin)
unset(_ARP_SHORT_DEVICE_VERSION)

#######################################################################################
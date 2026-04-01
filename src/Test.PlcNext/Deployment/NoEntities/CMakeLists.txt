cmake_minimum_required(VERSION 3.27)

project(NoEntities)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

################# create target #######################################################

set (WILDCARD_SOURCE *.cpp)
set (WILDCARD_HEADER *.h *.hpp *.hxx)

file(GLOB_RECURSE Headers src/${WILDCARD_HEADER} intermediate/code/${WILDCARD_HEADER})
file(GLOB_RECURSE Sources src/${WILDCARD_SOURCE} intermediate/code/${WILDCARD_SOURCE})
add_library(NoEntities SHARED ${Headers} ${Sources})

#######################################################################################

################# project include-paths ###############################################

target_include_directories(NoEntities
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

target_link_options(NoEntities PRIVATE LINKER:--no-undefined)

#######################################################################################

################# add link targets ####################################################

find_package(ArpDevice REQUIRED)
message(STATUS "ARP_DEVICE_VERSION=${ARP_DEVICE_VERSION}")
message(STATUS "ARP_DEVICE_SHORT_VERSION=${ARP_DEVICE_SHORT_VERSION}")
message(STATUS "ARP_DEVICE_SHORT_VERSION_BUILD=${ARP_DEVICE_SHORT_VERSION_BUILD}")

find_package(ArpProgramming REQUIRED)

target_link_libraries(NoEntities PRIVATE ArpDevice ArpProgramming)

#######################################################################################

################# install ############################################################

install(TARGETS NoEntities
    LIBRARY DESTINATION ${ARP_DEVICE}_${ARP_DEVICE_SHORT_VERSION_BUILD}/$<CONFIG>/lib
    ARCHIVE DESTINATION ${ARP_DEVICE}_${ARP_DEVICE_SHORT_VERSION_BUILD}/$<CONFIG>/lib
    RUNTIME DESTINATION ${ARP_DEVICE}_${ARP_DEVICE_SHORT_VERSION_BUILD}/$<CONFIG>/bin)

#######################################################################################
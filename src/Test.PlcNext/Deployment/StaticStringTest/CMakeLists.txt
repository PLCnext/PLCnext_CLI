cmake_minimum_required(VERSION 3.27)

project(StaticStringTest)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

################# create target #######################################################

file(GLOB_RECURSE Headers CONFIGURE_DEPENDS src/*.h src/*.hpp src/*.hxx intermediate/code/*.h intermediate/code/*.hpp intermediate/code/*.hxx)
file(GLOB_RECURSE Sources CONFIGURE_DEPENDS src/*.cpp intermediate/code/*.cpp)
add_library(StaticStringTest SHARED ${Headers} ${Sources})

#######################################################################################

################# project include-paths ###############################################

target_include_directories(StaticStringTest
    PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/intermediate/code>
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/src>)

#######################################################################################

################# include arp cmake module path #######################################

list(INSERT CMAKE_MODULE_PATH 0 "${ARP_TOOLCHAIN_CMAKE_MODULE_PATH}")

#######################################################################################

################# set RPATH to ORIGIN for thrid party libraries #######################
# Only by setting this is it possible to use libraries which are not already          #
# installed in the default install locations on the target.                           #
#######################################################################################

if(CMAKE_HOST_SYSTEM_NAME STREQUAL "Windows")
    # With Windows there is a known issue with CMake: https://gitlab.kitware.com/cmake/cmake/-/issues/21291
    # This is currently the only way to set the RPATH to $ORIGIN in Windows
    target_link_libraries(StaticStringTest PRIVATE "-Wl,-rpath,\$$ORIGIN")
else()
    set_target_properties(StaticStringTest PROPERTIES INSTALL_RPATH $ORIGIN)
endif()

#######################################################################################

################# set link options ####################################################
# WARNING: Without --no-undefined the linker will not check, whether all necessary    #
#          libraries are linked. When a library which is necessary is not linked,     #
#          the firmware will crash and there will be NO indication why it crashed.    #
#######################################################################################

target_link_options(StaticStringTest PRIVATE LINKER:--no-undefined)

#######################################################################################

################# add link targets ####################################################

find_package(ArpDevice REQUIRED)
find_package(ArpProgramming REQUIRED)

target_link_libraries(StaticStringTest PRIVATE ArpDevice ArpProgramming)

#######################################################################################

################# install ############################################################

string(REGEX REPLACE "^.*\\(([0-9]+\.[0-9]+\.[0-9]+\.[0-9]+).*$" "\\1" _ARP_SHORT_DEVICE_VERSION ${ARP_DEVICE_VERSION})
install(TARGETS StaticStringTest
    LIBRARY DESTINATION ${ARP_DEVICE}_${_ARP_SHORT_DEVICE_VERSION}/$<CONFIG>/lib
    ARCHIVE DESTINATION ${ARP_DEVICE}_${_ARP_SHORT_DEVICE_VERSION}/$<CONFIG>/lib
    RUNTIME DESTINATION ${ARP_DEVICE}_${_ARP_SHORT_DEVICE_VERSION}/$<CONFIG>/bin)
unset(_ARP_SHORT_DEVICE_VERSION)

#######################################################################################
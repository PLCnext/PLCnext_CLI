cmake_minimum_required(VERSION 3.27)

project(LibraryInfoTest)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

################# create target #######################################################

file(GLOB_RECURSE Headers CONFIGURE_DEPENDS src/*.h src/*.hpp src/*.hxx intermediate/code/*.h intermediate/code/*.hpp intermediate/code/*.hxx)
file(GLOB_RECURSE Sources CONFIGURE_DEPENDS src/*.cpp intermediate/code/*.cpp)
add_library(${PROJECT_NAME} SHARED ${Headers} ${Sources})

#######################################################################################

################# project include-paths ###############################################

target_include_directories(${PROJECT_NAME}
    PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/intermediate/code>
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/src>)

#######################################################################################

################# include arp cmake module path #######################################

list(INSERT CMAKE_MODULE_PATH 0 "${ARP_TOOLCHAIN_CMAKE_MODULE_PATH}")

#######################################################################################

################# set RPATH to ORIGIN for third party libraries #######################
# Only by setting this is it possible to use libraries which are not already          #
# installed in the default install locations on the target.                           #
#######################################################################################

set_target_properties(${PROJECT_NAME} PROPERTIES INSTALL_RPATH $ORIGIN)

#######################################################################################

################# set link options ####################################################
# WARNING: Without --no-undefined the linker will not check, whether all necessary    #
#          libraries are linked. When a library which is necessary is not linked,     #
#          the firmware will crash and there will be NO indication why it crashed.    #
#######################################################################################

target_link_options(${PROJECT_NAME} PRIVATE LINKER:--no-undefined)    
target_link_options(${PROJECT_NAME} PRIVATE LINKER:--as-needed)

#######################################################################################
                          
################# add link targets ####################################################

find_package(ArpDevice REQUIRED)
message(STATUS "ARP_DEVICE_VERSION=${ARP_DEVICE_VERSION}")
message(STATUS "ARP_DEVICE_SHORT_VERSION=${ARP_DEVICE_SHORT_VERSION}")
message(STATUS "ARP_DEVICE_SHORT_VERSION_BUILD=${ARP_DEVICE_SHORT_VERSION_BUILD}")


if(${ARP_VERSION_MAJOR} GREATER_EQUAL "25")
    find_package(Arp REQUIRED)
    target_link_libraries(${PROJECT_NAME} PRIVATE Arp::ALL)
else()
    find_package(ArpProgramming REQUIRED)
    target_link_libraries(${PROJECT_NAME} PRIVATE ArpDevice ArpProgramming)
endif()

#######################################################################################

################# install ############################################################

install(TARGETS ${PROJECT_NAME}
    LIBRARY DESTINATION ${ARP_DEVICE}_${ARP_DEVICE_SHORT_VERSION_BUILD}/$<CONFIG>/lib
    ARCHIVE DESTINATION ${ARP_DEVICE}_${ARP_DEVICE_SHORT_VERSION_BUILD}/$<CONFIG>/lib
    RUNTIME DESTINATION ${ARP_DEVICE}_${ARP_DEVICE_SHORT_VERSION_BUILD}/$<CONFIG>/bin)

#######################################################################################
cmake_minimum_required(VERSION 3.4)

project(Demo)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

################# create target #######################################################

set (WILDCARD_SOURCE *.cpp)
set (WILDCARD_HEADER *.h *.hpp *.hxx)

file(GLOB_RECURSE Headers ${WILDCARD_HEADER})
file(GLOB_RECURSE Sources ${WILDCARD_SOURCE})
add_library(Demo SHARED ${Headers} ${Sources})

#######################################################################################

################# project include-paths ###############################################

target_include_directories(Demo PRIVATE src intermediate/Code)

#######################################################################################

## necessary? see toolchain
list(INSERT CMAKE_MODULE_PATH 0 "${ARP_CMAKE_MODULE_PATH}")

################# add link targets ####################################################

find_package(ArpDevice REQUIRED)
find_package(ArpProgramming REQUIRED)

target_link_libraries(Demo PRIVATE ArpDevice ArpProgramming)

#######################################################################################

################# install ############################################################

## how to use staging prefix?
install(TARGETS Demo LIBRARY DESTINATION ${PROJECT_BINARY_DIR}/../../bin)

#######################################################################################

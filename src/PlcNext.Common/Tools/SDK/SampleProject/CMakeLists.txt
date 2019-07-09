cmake_minimum_required(VERSION 3.12)

project(MyProject)

add_library(MyProject SHARED a.cxx)
list(INSERT CMAKE_MODULE_PATH 0 "${ARP_TOOLCHAIN_CMAKE_MODULE_PATH}")

find_package(ArpDevice)
find_package(ArpProgramming)

target_link_libraries(MyProject PRIVATE ArpDevice ArpProgramming)
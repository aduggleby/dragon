#
# Dragon Build File
#

properties {
  # Version Properties
  $version =        "pre-alpha"

  # Directories
  $base_dir =       resolve-path .
  $lib_dir =        "$base_dir\lib"
  $build_dir =      "$base_dir\build" 
  $release_dir =    "$base_dir\release" 
  $tools_dir =      "$base_dir\tools"
  $src_dir =        "$base_dir\src"    
}

task default -depends Full

task Full -depends Header, Clean, Init, Compile, Test  { 
  
}

task Compile { 
  
}

task Test { 
  
}

task Clean { 
    "Clean build directory"
    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
    "Clean release directory"
    remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
}

task Init {
    "Creating build directory"
    new-item $release_dir -itemType directory  -ErrorAction SilentlyContinue | out-null 
    "Creating release directory"
    new-item $build_dir -itemType directory  -ErrorAction SilentlyContinue | out-null 
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}

task Header {
    "//////////////////////////////////////////////////////////////"
    "                                                              "
    "         _/                                                   " 
    "    _/_/_/  _/  _/_/    _/_/_/    _/_/_/    _/_/    _/_/_/    "
    " _/    _/  _/_/      _/    _/  _/    _/  _/    _/  _/    _/   "
    "_/    _/  _/        _/    _/  _/    _/  _/    _/  _/    _/    "
    " _/_/_/  _/          _/_/_/    _/_/_/    _/_/    _/    _/     "
    "                                  _/                          "
    "                             _/_/                             "
    " (c) 2011-2013 Dragon Team                  Version: $version "
    "                                                              "
    "//////////////////////////////////////////////////////////////"
}
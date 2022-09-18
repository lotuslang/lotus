# lotus

A toy language for personally exploring compilers and language design.

You can find some (somewhat outdated) notes and drafts about lotus in the [wiki](https://github.com/Blokyk/lotus/wiki), although a large amount of ideas are just floating around in my personal notes. In either case, lotus is an ever-changing thing, so most samples and document are subject to documentation-rot. For example, some features (such as enums or generic types) are currently severely limited or even not supported at all. Once I'm satisfied with the parser and front-end for lotus (e.g. good CLI, or the ability to process a *project* instead of a single file), I'll most likely write a sample project to test things on; but until then `test.lts` is the most up-to-date example of lotus code.

## Usage

Although lotus is nowhere-near usable, it is distributed with a very basic CLI, currently built with [`System.CommandLine`](https://github.com/dotnet/command-line-api/). It supports checking an input file for parsing errors, as well as output the parse tree in Graphviz/DOT format (optionally with const-coloring). It can also try to reconstruct the original source file solely from the parse tree. For more info, run `dotnet run --project ./src/Lotus.CLI/ -- -h`.
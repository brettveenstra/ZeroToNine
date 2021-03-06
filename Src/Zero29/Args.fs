﻿namespace Ploeh.ZeroToNine

open System
open Ploeh.ZeroToNine.Versioning

type Arg =
    | Assign of Version
    | AssignRank of Rank * int
    | Increment of Rank
    | ListVersions
    | ShowHelp
    | Unknown of string list

module Args =

    let private (|IsProperVersionString|_|) candidate =
        match Version.TryParse candidate with
        | (true, version) -> Some version
        | _ -> None

    let private (|IntegerGreaterThanOrEqualToZero|_|) candidate =
        match Int32.TryParse candidate with
        | (true, n) when n >= 0 -> Some n
        | _ -> None

    let Parse argv =
        match argv |> Seq.toList with
        | ["-l"] -> ListVersions
        | ["-a"; IsProperVersionString version] -> Assign version
        | ["-i"; "major"] -> Increment Rank.Major
        | ["-i"; "minor"] -> Increment Rank.Minor
        | ["-i"; "build"] -> Increment Rank.Build
        | ["-i"; "patch"] -> Increment Rank.Build
        | ["-i"; "revision"] -> Increment Rank.Revision
        | ["-a"; "major"; IntegerGreaterThanOrEqualToZero rankValue] -> AssignRank(Rank.Major, rankValue)
        | ["-a"; "minor"; IntegerGreaterThanOrEqualToZero rankValue] -> AssignRank(Rank.Minor, rankValue)
        | ["-a"; "build"; IntegerGreaterThanOrEqualToZero rankValue] -> AssignRank(Rank.Build, rankValue)
        | ["-a"; "patch"; IntegerGreaterThanOrEqualToZero rankValue] -> AssignRank(Rank.Build, rankValue)
        | ["-a"; "revision"; IntegerGreaterThanOrEqualToZero rankValue] -> AssignRank(Rank.Revision, rankValue)
        | ["-?"] -> ShowHelp
        | ["-h"] -> ShowHelp
        | [] -> ShowHelp
        | x -> Unknown(x)
        |> Seq.singleton

    let Usage =
        [
            "Zero29 <command> [<args>]"
            ""
            "-i <major|minor|build|patch|revision>"
            "   - Increments the specified component of each"
            "     Assembly Version and Assembly File Version attribute."
            "-a <major|minor|build|patch|revision> <version number>"
            "   - Assigns a particular Version number to the specified component of each"
            "     Assembly Version and Assembly File Version attribute."            
            "-a <version>"
            "   - Assigns a particular Version to all "
            "     Assembly Version and Assembly File Version attributes."
            "-l"
            "   - Lists the versions."
            "-? | -h"
            "   - Displays this help."
        ]

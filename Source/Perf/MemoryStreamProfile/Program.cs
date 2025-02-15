// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MemoryStreamProfile;

// Check if we should run a continuous grow test
if (args.Length > 0)
{
    if (string.Compare(args[0], "large", StringComparison.InvariantCultureIgnoreCase) == 0)
    {
        LargeFillProfile.RunProfile(args[1..]);
        return;
    }
    if (string.Compare(args[0], "grow", StringComparison.InvariantCultureIgnoreCase) == 0)
    {
        GrowProfile.RunProfile(args[1..]);
        return;
    }
    if (string.Compare(args[0], "wrap", StringComparison.InvariantCultureIgnoreCase) == 0)
    {
        WrapProfile.RunProfile(args[1..]);
        return;
    }
}
// Otherwise run a file profile test
FillProfile.RunProfile(args);

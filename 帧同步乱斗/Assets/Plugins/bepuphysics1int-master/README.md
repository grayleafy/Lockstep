# BEPUphysicsint v1

*This is a fork of the awesome [bepuphysics v1](https://github.com/bepu/bepuphysics1)*

BEPUphysics is a pure C# 3D physics library by [BEPU](http://bepuphysics.com). It's fast and has a bunch of cool features like constraints, terrain, static and instanced meshes, continuous collision detection, custom collision rules, vehicles, easy multithreading, yadda yadda yadda.

BEPUphysicsint is a fork that uses the [FixedMath.net](https://github.com/asik/FixedMath.Net) fixed-point integer math instead of floats. This ensures full cross-platform determinism of the physics simulation.

This fork is (almost 100%) compatible with BEPUphysics, so please refer to the BEPUphysics [documentation page](Documentation/Documentation.md) for reference.

# Known issues
 * The value range of fixed point number is much more limited than float. As a result, the extent of the physics world should be limited to about 1000 on each axis. When the *CHECKMATH* compilation symbol is specified (e.g. in DEBUG builds), the math library will throw overflow exceptions.
 * Performance. Currently this fork is about 4 times slower than the float version. Hopefully this will be improved soon, but the fixed-point version will definitely remain slower than the float version.

# Determinism caveats
 * Multithreading has to be disabled, otherwise the simulation will not be deterministic. See [InternalMultithreading](Documentation/InternalMultithreading.md#3a--determinism) for reference.
 * Do not use floating point values when setting up the physics world. This probably also applies for collision meshes!

# 
For discussions about BEPUphysics, please head to the [BEPUphysics forums](https://forum.bepuentertainment.com).

If you're looking for more BEPU-related stuff, head to the [main BEPU website](http://bepuphysics.com).

If you're feeling angelic, you can throw money at the creators of BEPUphysics:

![](Documentation/images/readme/angelduck.png)

[![](Documentation/images/readme/throw%20money.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=contact%40bepuentertainment%2ecom&lc=US&item_name=BEPUphysics&no_note=0¤cy_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHostedGuest)

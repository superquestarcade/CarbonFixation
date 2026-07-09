namespace Utility
{
	// functions for working with bit masks

public static class BitmaskUtils
{
    //--------------- ulong --------------------//

    // returns the mask with the specified bit set to one.
    public static ulong SetBit(ulong mask, byte index)
    {
        return (1u << index) | mask;
    }

    // returns the mask with the specified bit set to zero.
    public static ulong UnsetBit(ulong mask, byte index)
    {
        return mask & ~(1u << index);
    }

    // returns the mask with the specified bit set to one or zero based on enable.
    public static ulong ToggleBit(ulong mask, byte index, bool enable)
    {
        return enable ? SetBit(mask, index) : UnsetBit(mask, index);
    }

    // returns true if the specified bit in the given mask is one.
    public static bool IsBitSet(ulong mask, byte index)
    {
        return (mask & (1UL << index)) != 0;
    }

    // returns a mask where all the bits are set that are in either mask1 or mask2.
    public static ulong SetBits(ulong mask1, uint mask2)
    {
        return mask1 | mask2;
    }

    // returns mask1 where all the bits that are 1 in mask2 are set to 0.
    public static ulong UnsetBits(ulong mask1, uint mask2)
    {
        return mask1 & ~mask2;
    }

    // sets or unsets bits based on enable.
    public static ulong ToggleBits(ulong mask1, uint mask2, bool enable)
    {
        return enable ? SetBits(mask1, mask2) : UnsetBits(mask1, mask2);
    }

    //--------------------- uint --------------------//

    // returns the mask with the specified bit set to one.
    public static uint SetBit(uint mask, byte index)
    {
        return (1u << index) | mask;
    }

    // returns the mask with the specified bit set to zero.
    public static uint UnsetBit(uint mask, byte index)
    {
        return mask & ~(1u << index);
    }

    // returns the mask with the specified bit set to one or zero based on enable.
    public static uint ToggleBit(uint mask, byte index, bool enable)
    {
        return enable ? SetBit(mask, index) : UnsetBit(mask, index);
    }

    // returns true if the specified bit in the given mask is one.
    public static bool IsBitSet(uint mask, byte index)
    {
        return (mask & (1 << index)) != 0;
    }

    // returns a mask where all the bits are set that are in either mask1 or mask2.
    public static uint SetBits(uint mask1, uint mask2)
    {
        return mask1 | mask2;
    }

    // returns mask1 where all the bits that are 1 in mask2 are set to 0.
    public static uint UnsetBits(uint mask1, uint mask2)
    {
        return mask1 & ~mask2;
    }

    // sets or unsets bits based on enable.
    public static uint ToggleBits(uint mask1, uint mask2, bool enable)
    {
        return enable ? SetBits(mask1, mask2) : UnsetBits(mask1, mask2);
    }

    //--------------------- byte --------------------//

    // returns the mask with the specified bit set to one.
    public static byte SetBit(byte mask, byte index)
    {
        return (byte)((1u << index) | mask);
    }

    // returns the mask with the specified bit set to zero.
    public static byte UnsetBit(byte mask, byte index)
    {
        return (byte)(mask & ~(1u << index));
    }

        // returns the mask with the specified bit set to one or zero based on enable.
    public static byte ToggleBit(byte mask, byte index, bool enable)
    {
        return enable ? SetBit(mask, index) : UnsetBit(mask, index);
    }

    // returns true if the specified bit in the given mask is one.
    public static bool IsBitSet(byte mask, byte index)
    {
        return (mask & ((byte)1 << index)) != 0;
    }

    // returns a mask where all the bits are set that are in either mask1 or mask2.
    public static byte SetBits(byte mask1, byte mask2)
    {
        return (byte)(mask1 | mask2);
    }

    // returns a mask where all the bits that are one is mask2 become zero in mask1.
    public static byte UnsetBits(byte mask1, byte mask2)
    {
        return (byte)(mask1 & ~mask2);
    }

    // sets or unsets bits based on enable.
    public static byte ToggleBits(byte mask1, byte mask2, bool enable)
    {
        return enable ? SetBits(mask1, mask2) : UnsetBits(mask1, mask2);
    }
}
}
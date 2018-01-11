namespace AL {

public class LCG {
	uint a, c, s, m;
	public LCG(uint a, uint c, uint m, uint s) {
		this.a = a;
		this.c = c;
		this.m = m;
		this.s = s;
	}
	public uint next() {
		return s = (s * a + c) & m;
	}
	static public LCG NumericalRecipes(uint s) {
		return new LCG(1664525, 1013904223, 0xFFFFFFFF, s);
	}
	static public LCG glibc(uint s) {
		return new LCG(1103515245, 12345, 0xFFFFFFFF, s);
	}
};

public class LCGMask {
	uint r = 0, m;
	LCG c;
	public LCGMask(LCG c) {
		this.c = c;
	}
	public byte[] mask(byte[] b) {
		for (int i = 0; i < b.Length; ++i) {
			if (r == 0) { m = c.next(); r = sizeof(uint); }
			b[i] ^= (byte)(m & 0xFF);
			m >>= 8;
			--r;
		}
		return b;
	}
	static public LCGMask NumericalRecipes(uint s) {
		return new LCGMask(LCG.NumericalRecipes(s));
	}
	static public LCGMask glibc(uint s) {
		return new LCGMask(LCG.glibc(s));
	}
};

}

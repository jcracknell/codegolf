/** Facility for computation of the Levenshtein distance between indexed sequences `a` and `b`; the number of
  * single-element insertions, deletions and substitutions required to transform `a` into `b`. */
object LevenshteinDistance {
  /** Computes the Levenshtein distance between indexed sequences `a` and `b`; the number of single-element insertions,
    * deletions and substitutions required to transform `a` into `b`.
    * Most often used to compute the distance between two strings.
    * Can optionally be provided with a method for comparing sequence elements in the event that the default equality
    * semantics are undesirable.
    *
    * @param a the first sequence.
    * @param b the second sequence.
    * @param eq optional method which can be used to override comparison semantics.
    * @tparam T the element type of sequences `a` and `b`.
    * @return the Levenshtein distance between sequences `a` and `b`.
    */
  def compute[T](a:IndexedSeq[T], b:IndexedSeq[T], eq:(T, T) => Boolean = { (ae:T, be:T) => ae == be }):Int = {
    // This is a very non-idiomatic implementation, for performance reasons
    val al = a.length; val bl = b.length;
    if(0 == al) return bl;
    if(0 == bl) return al;

    val ds = bl + 1;
    var d0 = Array.range(0, ds);
    var d1 = new Array[Int](ds);

    var ai = 1; var aii = 0;
    while(ai <= al) {
      val ae = a(aii); // Cache the element of `a` under consideration
      d1(0) = ai;
			var bii = 0; var bi = 1;
      while(bi <= bl) {
        d1(bi) =
          if(eq(ae, b(bii))) d0(bii)
          else {
            // This is a highly optimized `min` implementation
            val x = d0(bii); val y = d0(bi); val z = d1(bii);
            (if(x < y) (if(x < z) x else z) else (if(y < z) y else z)) + 1;
          }

        bii = bi; bi += 1;
      }

      // Swap `d1` and `d0` so that `d1` becomes the 'previous' row
      val dd = d0; d0 = d1; d1 = dd;
      aii = ai; ai += 1;
    }

    // The result is the last element of `d0`, which *was* `d1` before it was swapped
    d0(bl)
  }
}

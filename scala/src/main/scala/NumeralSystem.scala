/** @define NS `NumeralSystem`
 *  @define ns numeral system
 *
 *  @author James Cracknell
 *
 *  Trait describing a $ns capable of encoding and decoding integer values to and from
 *  a corresponding string representation.
 */
trait NumeralSystem {
  /** Attempts to decode the provided `String` value `s` according to the $NS, returning the
   *  corresponding `Int` value if successful, `None` otherwise.
   *
   *  @param s The `String` value to be decoded.
   *  @return The corresponding `Int` value if successful, `None` otherwise.
   */
  def decode(s:String):Option[Int]

  /** Attempts to encode the provided `Int` value `i` according to the $NS, returning a corresponding
   *  `String` representation if `i` can be encoded, `None` otherwise.
   *
   *  @param i The `Int` value to be encoded.
   *  @return A corresponding `String` representation if `i` can be encoded, `None` otherwise.
   */
  def encode(i:Int):Option[String]
}

object NumeralSystem {
  /** `NumeralSystem` encoding positive integers using roman letters 'a' through 'z'. */
  object Alpha extends NumeralSystem {
    private val CharValues = (('a' to 'z') ++ ('A' to 'Z')).map(c => (c -> (c & 31))).toMap

    def decode(s:String):Option[Int] =
      (Option(0) /: s) { (m,c) =>
        m match {
          case Some(x) => CharValues.get(c) map { _ + 26 * x }
          case None => m
        }
      }

    def encode(i:Int):Option[String] =
      if(1 > i) Option.empty[String]
      else {
        val ds = Iterator.iterate(i-1)(_/26 - 1).takeWhile(_ >= 0)
        Some(((ds :\ new StringBuilder) { (x, sb) => sb + ('a'+x%26).toChar }).toString)
      }
  }

  /** `NumeralSystem` encoding integers as arabic numerals, the numeral system with which
   *  most people are familiar. */
  object Arabic extends NumeralSystem {
    def decode(s:String): Option[Int] =
      try { Some(Integer.parseInt(s)) }
      catch { case _:Throwable => Option.empty[Int] }

    def encode(i:Int): Option[String] = Some(i.toString)
  }
}

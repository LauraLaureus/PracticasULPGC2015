using UnityEngine;
using System.Collections;

public interface Heuristic<E>  {

	float apply(E obj);
}

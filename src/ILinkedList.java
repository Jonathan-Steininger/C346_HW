public interface ILinkedList<T> {
    /**
     * Get the next list element
     * @return the next element
     */
    MyLinkedList<T> next();

    /**
     * Gets the last element in the list
     * @return the node at the end of the list
     */
    MyLinkedList<T> last();

    /**
     * Get the element n elements down the list
     * @param n the number of elements to skip
     * @return the element n away
     */
    MyLinkedList<T> after(int n);

    /**
     * Removes the next element (sets null)
     * @return the previously next element
     */
    MyLinkedList<T> detach();

    /**
     * Gets the current value
     * @return the current value
     */
    T getCurrent();

    /**
     * Sets the value of this node
     * @param value the new value
     */
    void setCurrent(T value);

    /**
     * Sets the next element in the list
     * @param next the next element
	 *
	 * Example: (1->2->3).setNext(4) => 1->4
     */
    void setNext(T next);

    /**
     * Sets the next element after this current element
     * @param next the next element
	 *
	 * Example: (1->2->3).appnd(4) => 1->2->3->4
     */
    void append(T next);

    /**
     * Adds the current list as the next of newFirst
     * @param newFirst the new head of the list
	 *
	 * Example: (1->2->3).insert(4) => 4->1->2->3
     */
    void insert(T newFirst);
}
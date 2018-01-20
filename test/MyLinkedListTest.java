/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 *
 * @author jonat
 */
public class MyLinkedListTest {
    
    public MyLinkedListTest() {
    }
    
    @BeforeClass
    public static void setUpClass() {
    }
    
    @AfterClass
    public static void tearDownClass() {
    }
    
    @Before
    public void setUp() {
    }
    
    @After
    public void tearDown() {
    }

    /**
     * Test of next method, of class MyLinkedList.
     */
    @Test
    public void testNext() {
        System.out.println("next");
        MyLinkedList instance = new MyLinkedList();
        instance.add(1);
        instance.add(2);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(2);
        MyLinkedList result = instance.next();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of last method, of class MyLinkedList.
     */
    @Test
    public void testLast() {
        System.out.println("last");
        MyLinkedList instance = new MyLinkedList();
        instance.add(1);
        instance.add(2);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(2);
        MyLinkedList result = instance.last();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of after method, of class MyLinkedList.
     */
    @Test
    public void testAfter() {
        System.out.println("after");
        int n = 2;
        MyLinkedList instance = new MyLinkedList();
        
        instance.add(1);
        instance.add(2);
        instance.add(3);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(3);
        
        MyLinkedList result = instance.after(n);
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of detach method, of class MyLinkedList.
     */
    @Test
    public void testDetach() {
        System.out.println("detach");
        MyLinkedList instance = new MyLinkedList();
        
        instance.add(1);
        instance.add(2);
        instance.add(3);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(2);
        
        
        MyLinkedList result = instance.detach();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of getCurrent method, of class MyLinkedList.
     */
    @Test
    public void testGetCurrent() {
        System.out.println("getCurrent");
        MyLinkedList instance = new MyLinkedList();
        instance.add(1);
        instance.add(2);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(1);
        
        Object result = instance.getCurrent();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of setCurrent method, of class MyLinkedList.
     */
    @Test
    public void testSetCurrent() {
        System.out.println("setCurrent");
        Object value = 3;
        MyLinkedList instance = new MyLinkedList();
        
        instance.add(1);
        instance.add(2);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(3);
        instance.setCurrent(value);
        Object result = instance.getCurrent();
        assertEquals(expResult, result);

        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of setNext method, of class MyLinkedList.
     */
    @Test
    public void testSetNext() {
        System.out.println("setNext");
        Object next = 3;
        MyLinkedList instance = new MyLinkedList();
        instance.add(1);
        instance.add(2);
        instance.setNext(3);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(3);
        Object result = instance.next();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of append method, of class MyLinkedList.
     */
    @Test
    public void testAppend() {
        System.out.println("append");
        Object next = 4;
        MyLinkedList instance = new MyLinkedList();
        instance.add(1);
        instance.add(2);
        instance.add(3);
        instance.append(next);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(4);
        Object result = instance.last();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of insert method, of class MyLinkedList.
     */
    @Test
    public void testInsert() {
        System.out.println("insert");
        Object newFirst = 0;
        MyLinkedList instance = new MyLinkedList();
        instance.add(1);
        instance.add(2);
        instance.insert(newFirst);
        MyLinkedList expResult = new MyLinkedList();
        expResult.add(0);
        Object result = instance.getCurrent();
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }
}
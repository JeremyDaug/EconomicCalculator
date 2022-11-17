pub mod objects;

pub fn add(left: usize, right: usize) -> usize {
    left + right
}

#[cfg(test)]
mod tests {

    use std::collections::HashMap;

    use crate::objects::product::Product;

    use super::*;
    use super::objects::pop::Pop;

    #[test]
    fn it_works() {
        let testDict = HashMap::<Product, f64>::new();



        let result = add(2, 2);
        assert_eq!(result, 4);
    }
}

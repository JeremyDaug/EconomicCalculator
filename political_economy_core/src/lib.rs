pub mod objects;
pub mod data_manager;

pub fn add(left: usize, right: usize) -> usize {
    left + right
}

#[cfg(test)]
mod tests {

    use std::collections::HashMap;

    use crate::objects::product::Product;

    use super::*;

    #[test]
    fn it_works() {
        let testDict = HashMap::<Product, f64>::new();



        let result = add(2, 2);
        assert_eq!(result, 4);
    }
}

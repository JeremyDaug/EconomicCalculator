#[derive(Debug)]
struct Want {
    Id: u32,
    Name: String,
    Description: String,

}

impl Want {
    fn new() -> Want {
        Want{
            Id = 0,
            Name = "Rest",
            Description = "The desire to relax and recuperate, using time to do nothing.",

        }
    }

    fn my_print(&self) {
        println!("{self.Name}: {self.Description}");
    }
}

fn build_want(Id: String, Name: String, Description: String) -> Want {
    Want {
        Id,
        Name,
        Description
    }
}
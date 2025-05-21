GENERATE_LIST = [
    {
        name: 'ValueList',
        input: 1,
        outputs: [2, 3, 4, 8, 16, 32, 64, 128, 256, 512],
    },
]
INPUT_DIR = '.'
OUTPUT_DIR = 'Generated'

begin
    require 'fileutils'

    # Delete generated directory
    FileUtils.rm_rf(OUTPUT_DIR)

    # Generate each entry
    for generate_entry in GENERATE_LIST do
        generate_input = generate_entry[:input]
        # Get directory in input directory
        input_dir = File.join(INPUT_DIR, generate_entry[:name] + generate_input.to_s)
        # Generate each output
        for generate_output in generate_entry[:outputs] do
            # Get directory in generated directory
            output_dir = File.join(OUTPUT_DIR, generate_entry[:name] + generate_output.to_s)
            # Copy input directory to generated directory
            FileUtils.mkdir_p(output_dir)
            FileUtils.cp_r(File.join(input_dir, '.'), output_dir)
            # Find & replace
            for file in Dir.children(output_dir) do
                # Find & replace file name
                renamed_file = file.gsub(generate_input.to_s, generate_output.to_s)
                full_renamed_file = File.join(output_dir, renamed_file)
                File.rename(File.join(output_dir, file), full_renamed_file)
                # Find & replace file contents
                File.write(full_renamed_file, File.read(full_renamed_file)
                    .gsub(generate_entry[:name] + generate_input.to_s, generate_entry[:name] + generate_output.to_s)
                    .gsub("Capacity = " + generate_input.to_s, "Capacity = " + generate_output.to_s)
                )
            end
        end
    end
rescue => ex
    # Output exception
    puts "#{ex.class}: #{ex.message}"
    puts ex.backtrace.join("\n")
end

# End of script
puts("All done.")
gets()